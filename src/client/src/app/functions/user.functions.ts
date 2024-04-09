import { Result } from "../models/components/Result";
import { UserPermission } from "../models/components/UserPermission";
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

export function tryMapUser(name: string, email: string, isAdmin: boolean, permissions: UserPermission[]): Result<IAppUserDto> {
    const validate = (x: string, error: string) =>
        !x || x.length === 0 ? error : '';

    const errors = [
        validate(name, 'Name cannot be empty'),
        validate(email, 'Email cannot be empty'),
    ]
    .filter(x => x.length > 0);

    const result: Result<IAppUserDto> = {
        x: {
            name: name,
            email: email,
            isAdmin: isAdmin,
            permissionsMap: mapPermissions(permissions)
        } as IAppUserDto,
        errors: errors
    }

    return result;
}

function mapPermissions(xs: UserPermission[]) {
    const permissionMap: { [key: string]: string[] } = {};
    
    xs.forEach(x => {
        const ps = x.permissions
            .filter(p => p.hasPermission)
            .map(p => p.name);

        permissionMap[x.section] = ps;
    });

    return permissionMap;
}