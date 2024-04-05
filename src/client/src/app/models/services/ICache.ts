export interface ICache {
    data: unknown;
    expiresAt: number;
}

export type CacheFunctions<T> = {
    store: (x: T) => void;
    retrieve: () => T | null;
    clear: () => void;
}