// Account type — a display-only preference (Personal / Business / Project).
// It labels the profile; it changes no mechanics anywhere. The wire value is
// the short lowercase name (preference value ≤512 chars, kebab-friendly),
// the long label is presentation only.

export const accountTypeNames = ['personal', 'business', 'project'] as const
export type AccountTypeName = (typeof accountTypeNames)[number]

/** Display labels — what the header subtitle and the profile sheet show */
export const accountTypeLabels: Record<AccountTypeName, string> = {
  personal: 'Personal Account',
  business: 'Business Account',
  project: 'Project Account',
}

/** True when value is a real account-type name — used before trusting stored data */
export function isAccountTypeName(value: string): value is AccountTypeName {
  return (accountTypeNames as readonly string[]).includes(value)
}

/** Label for a stored value; unknown values fall back to the default label */
export function labelOfAccountType(value: AccountTypeName): string {
  return accountTypeLabels[value]
}
