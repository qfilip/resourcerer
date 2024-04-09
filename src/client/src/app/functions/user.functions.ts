import { jwtClaimKeys, permissionsMap } from "../models/dtos/constants";
import { IAppUserDto } from "../models/dtos/interfaces";

export function parseJwt(jwt: string) {
    const body = jwt.split('.')[1];
    const jwtObj = JSON.parse(atob(body));

    const now = (new Date()).getTime();
    const issuedAt = new Date(jwtObj[jwtClaimKeys.issuedAt]).getTime();
    
    const lifetime = new Date(Date.UTC(jwtObj['exp'] as number)).getTime();
    const utcNow = new Date(Date.UTC(now)).getTime();
    
    const expired = (issuedAt + lifetime) <= utcNow;
    
    const userDto = {
        id: jwtObj[jwtClaimKeys.id],
        name: jwtObj[jwtClaimKeys.name],
        displayName: jwtObj[jwtClaimKeys.displayName],
        email: jwtObj[jwtClaimKeys.email],
        isAdmin: jwtObj[jwtClaimKeys.isAdmin].toLowerCase() === 'true' ? true : false,
        company: {
            id: jwtObj[jwtClaimKeys.companyId],
            name: jwtObj[jwtClaimKeys.companyName]
        },
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
        expired: expired
    };
}