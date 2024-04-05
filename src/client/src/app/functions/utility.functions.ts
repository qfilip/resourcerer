export function dictToArray(dict: { [key: string]: any[] }) {
    let xs = [];
    for(const key in dict) {
        xs.push({
            key: key,
            values: dict[key]
        });
    }
    console.log(xs)
    return xs;
}