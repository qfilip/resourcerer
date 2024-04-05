import { permissionsMap } from "../models/dtos/constants";
import { IAppUserDto } from "../models/dtos/interfaces";

export function parseJwt(jwt: string) {
    const body = jwt.split('.')[1];
    const jwtObj = JSON.parse(atob(body));

    const expiresAt = jwtObj['exp'];

    const userDto = {
        id: jwtObj['userid'],
        name: jwtObj['sub'],
        jwt: jwt,
        permissionsMap: {}
    } as IAppUserDto;

    for(const key in permissionsMap) {
        const hasSection = jwtObj[key];
        
        if(hasSection) {
            userDto.permissionsMap[key] = jwtObj[key];
        }
    }

    return {
        dto: userDto,
        expiresAt: expiresAt
    };
}