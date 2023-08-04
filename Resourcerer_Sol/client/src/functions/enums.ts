export function getEnumKeys(enumerator: any) {
    let keys: string[] = [];

    for(let e in enumerator) {
        if(isNaN(parseInt(e))) {
            keys.push(e);
        }
    }

    return keys;
}