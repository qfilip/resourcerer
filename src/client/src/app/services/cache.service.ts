import { CacheFunctions } from "../models/services/ICache";

export abstract class CacheService {
    public abstract register<T>(key: string, expiresAfter: number): CacheFunctions<T>;
    protected abstract store<T>(key: string, data: T, expiresAfter: number): void;
    protected abstract retrieve<T>(key: string): T | null;
    protected abstract clear(key: string): void;
}