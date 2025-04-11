export type UserPermission = { 
    section: string;
    permissions: {
        name: string;
        hasPermission: boolean;
    }[]
 }