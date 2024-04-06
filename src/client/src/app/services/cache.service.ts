export abstract class CacheService {
    protected abstract store<T>(key: string, data: T, expiresAfter: number): void;
    protected abstract retrieve<T>(key: string): T | null;
    protected abstract clear(key: string): void;
}