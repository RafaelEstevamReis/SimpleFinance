import { describe, expect, it } from 'vitest'
import { buildCreateTransferRequest, buildUpdateTransferRequest, pairHeading } from './transfers'

const createForm = {
  sourceWalletId: 1,
  sourceCategoryId: 0,
  destinationWalletId: 2,
  destinationCategoryId: 0,
  description: 'move to savings',
  value: '300',
  dueDate: '2026-11-08T12:00:00Z',
  paymentDate: '2026-11-08T12:00:00Z',
  paid: true,
  paymentDetails: '',
}

describe('buildCreateTransferRequest', () => {
  it('builds the wire body with a positive value and empty details nulled', () => {
    const { request, error } = buildCreateTransferRequest(createForm)
    expect(error).toBeNull()
    expect(request).toEqual({
      sourceWalletId: 1,
      sourceCategoryId: 0,
      destinationWalletId: 2,
      destinationCategoryId: 0,
      description: 'move to savings',
      value: 300,
      dueDate: '2026-11-08T12:00:00Z',
      paymentDate: '2026-11-08T12:00:00Z',
      paid: true,
      paymentDetails: null,
    })
  })

  it('trims description and keeps payment details when set', () => {
    const { request } = buildCreateTransferRequest({ ...createForm, description: '  rent split  ', paymentDetails: ' pix ' })
    expect(request!.description).toBe('rent split')
    expect(request!.paymentDetails).toBe('pix')
  })

  it('refuses same wallet as source and destination', () => {
    const { request, error } = buildCreateTransferRequest({ ...createForm, destinationWalletId: 1 })
    expect(request).toBeNull()
    expect(error).toBe('Source and destination must be different wallets')
  })

  it('refuses a missing wallet on either side', () => {
    expect(buildCreateTransferRequest({ ...createForm, sourceWalletId: 0 }).error).toBe('Source wallet is required')
    expect(buildCreateTransferRequest({ ...createForm, destinationWalletId: 0 }).error).toBe('Destination wallet is required')
  })

  it('refuses empty description', () => {
    expect(buildCreateTransferRequest({ ...createForm, description: '   ' }).error).toBe('Description is required')
  })

  it('refuses zero, negative and non-numeric values', () => {
    expect(buildCreateTransferRequest({ ...createForm, value: '0' }).error).toBe('Value cannot be zero')
    expect(buildCreateTransferRequest({ ...createForm, value: '-10' }).error).toBe('Value is sent positive — the pair carries both sides')
    expect(buildCreateTransferRequest({ ...createForm, value: 'abc' }).error).toBe('Value must be a number')
  })
})

const updateForm = {
  description: 'move to savings',
  value: '300',
  paidValue: '300',
  status: 'Paid' as const,
  dueDate: '2026-11-08T12:00:00Z',
  paymentDate: '2026-11-08T12:00:00Z',
  paymentDetails: '',
}

describe('buildUpdateTransferRequest', () => {
  it('builds the wire body: magnitudes positive, the server re-signs both legs', () => {
    const { request, error } = buildUpdateTransferRequest(updateForm)
    expect(error).toBeNull()
    expect(request).toEqual({
      dueValue: 300,
      paidValue: 300,
      dueDate: '2026-11-08T12:00:00Z',
      paymentDate: '2026-11-08T12:00:00Z',
      description: 'move to savings',
      paymentDetails: null,
      status: 'Paid',
    })
  })

  it('unpaid forces paidValue 0 whatever the field says', () => {
    const { request } = buildUpdateTransferRequest({ ...updateForm, status: 'Unpaid', paidValue: '999' })
    expect(request!.paidValue).toBe(0)
  })

  it('refuses zero, negative and non-numeric values', () => {
    expect(buildUpdateTransferRequest({ ...updateForm, value: '0' }).error).toBe('Value cannot be zero')
    expect(buildUpdateTransferRequest({ ...updateForm, value: '-5' }).error).toBe('Value is sent positive — the pair carries both sides')
    expect(buildUpdateTransferRequest({ ...updateForm, value: 'x' }).error).toBe('Value must be a number')
  })

  it('refuses a negative paid value when status is Paid', () => {
    expect(buildUpdateTransferRequest({ ...updateForm, paidValue: '-1' }).error).toBe('Paid value is sent positive — the pair carries both sides')
  })

  it('refuses empty description', () => {
    expect(buildUpdateTransferRequest({ ...updateForm, description: '' }).error).toBe('Description is required')
  })
})

describe('pairHeading', () => {
  it('states the frozen direction of the pair', () => {
    expect(pairHeading('Main Account', 'Dollars')).toBe('Main Account → Dollars')
  })
})
