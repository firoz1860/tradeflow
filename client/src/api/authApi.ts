import { api } from './client'
import type { AuthSession } from '../types'

export const register = (email: string, password: string) => api<AuthSession>('/api/auth/register', undefined, { method: 'POST', body: JSON.stringify({ email, password }) })
export const login = (email: string, password: string) => api<AuthSession>('/api/auth/login', undefined, { method: 'POST', body: JSON.stringify({ email, password }) })
