export interface ICache {
    data: unknown;
    storedAt: number;
    expiresAfter: number;
}

export type CacheFunctions = {
    store: <T>(x: T) => void;
    retrieve: <T>() => T | null;
}