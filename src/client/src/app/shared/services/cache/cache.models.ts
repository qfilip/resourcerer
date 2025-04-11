import { Observable } from "rxjs";

export interface CachedData {
    data: unknown;
    expiresAt?: number;
}

export type CacheFunctions<T> = {
    retrieve: () => Observable<T>;
    getCache(): CachedData;
    setCache<T>(data: T): void;
    clear: () => void;
}