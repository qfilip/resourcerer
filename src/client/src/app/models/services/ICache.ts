export interface ICache {
    data: unknown;
    expiresAt: number;
}

export type CacheFunctions = {
    store: <T>(x: T) => void;
    retrieve: <T>() => T | null;
    clear: () => void;
}