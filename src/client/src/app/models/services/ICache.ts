import { Observable } from 'rxjs';

export interface ICache {
    data: unknown;
    expiresAt: number;
}

export type CacheFunctions<T> = {
    retrieve: () => Observable<T>;
    getCache(): ICache;
    setCache<T>(data: T): void;
    clear: () => void;
}