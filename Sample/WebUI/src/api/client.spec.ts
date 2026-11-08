import { describe, expect, it, vi } from 'vitest'
import { ApiClient, ApiError } from './client'

function mockFetch(status: number, body: unknown | null): Promise<Response> {
  return Promise.resolve({
    ok: status >= 200 && status < 300,
    status,
    statusText: 'Status',
    json: () =>
      body === null
        ? Promise.reject(new Error('no body'))
        : Promise.resolve(body),
  } as Response)
}

describe('ApiClient error shaping', () => {
  it('400 ProblemDetails becomes bad-request with detail', async () => {
    vi.stubGlobal('fetch', vi.fn(() =>
      mockFetch(400, { title: 'Invalid', detail: 'Category not found' }),
    ))
    const client = new ApiClient('http://x', 'k')
    await expect(client.account()).rejects.toSatisfy((e: unknown) => {
      const err = e as ApiError
      return err.kind === 'bad-request' && err.detail === 'Category not found'
    })
  })

  it('401 becomes unauthorized — drives back-to-Connect', async () => {
    vi.stubGlobal('fetch', vi.fn(() => mockFetch(401, null)))
    const client = new ApiClient('http://x', 'k')
    await expect(client.account()).rejects.toSatisfy((e: unknown) =>
      (e as ApiError).kind === 'unauthorized')
  })

  it('network failure becomes unreachable', async () => {
    vi.stubGlobal('fetch', vi.fn(() => Promise.reject(new Error('ECONNREFUSED'))))
    const client = new ApiClient('http://x')
    await expect(client.hello()).rejects.toSatisfy((e: unknown) =>
      (e as ApiError).kind === 'unreachable')
  })

  it('404 with JSON body becomes not-found, not not-a-webapi', async () => {
    vi.stubGlobal('fetch', vi.fn(() => mockFetch(404, { title: 'Not Found' })))
    const client = new ApiClient('http://x')
    await expect(client.hello()).rejects.toSatisfy((e: unknown) =>
      (e as ApiError).kind === 'not-found')
  })

  it('200 without ok body shape passes through to caller', async () => {
    vi.stubGlobal('fetch', vi.fn(() => mockFetch(200, { message: 'Hello World' })))
    const client = new ApiClient('http://x/')
    const hello = await client.hello()
    expect(hello.message).toBe('Hello World')
  })

  it('baseUrl trailing slash is trimmed — one canonical form', async () => {
    vi.stubGlobal('fetch', vi.fn(() => mockFetch(200, {})))
    const client = new ApiClient('http://x:8091///')
    expect(client.baseUrl).toBe('http://x:8091')
  })

  it('Key travels as X-Api-Key header', async () => {
    const spy = vi.fn(() => mockFetch(200, {}))
    vi.stubGlobal('fetch', spy)
    const client = new ApiClient('http://x', 'my-key')
    await client.account()
    const [, init] = spy.mock.calls[0] as [string, RequestInit]
    expect((init.headers as Record<string, string>)['X-Api-Key']).toBe('my-key')
  })
})
