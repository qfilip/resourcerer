<script lang="ts">
    import { getEnumKeys } from "../../functions/enums";
    import { ePermission, ePermissionSection } from "../../interfaces/dtos/enums";
    import type { IAppUserDto } from "../../interfaces/dtos/interfaces";

    export let user: IAppUserDto;

    let lookup: { [key: string]: { sectionIdx: number,  permissionMap: { [key: string]: number } } } = {};
    let allPermissions = getAllPermissions();

    function getAllPermissions() {
        lookup = {};

        return getEnumKeys(ePermissionSection).map((s, si) => {
            lookup[s] = {
                sectionIdx: si,
                permissionMap: {}
            };

            const permissions = getEnumKeys(ePermission).map((p, pi) => {
                lookup[s].permissionMap[p] = pi;
                return { permission: p, hasPermission: false }
            });
            
            return {
                section: s,
                permissions: permissions
            }
        });
    }

    $: {
        if(user) {
            const sections = Object.keys(user.permissions);
            const permissions = getEnumKeys(ePermission);
            
            sections.forEach(s => {
                const permissionLevel = user.permissions[s];
                permissions.forEach(p => {
                    if((ePermission[p] & permissionLevel) > 0) {
                        const sGroup = lookup[s];
                        const pIdx = sGroup.permissionMap[p];

                        allPermissions[sGroup.sectionIdx].permissions[pIdx].hasPermission = true;
                    }
                });
            });
        
        }
    }
    
    
</script>


<div>
    {#each allPermissions as ps}
        <div>
            {ps.section}
        </div>
        <div class="permission-box">
            {#each ps.permissions as p}
                <label>
                    {p.permission}
                    <input type="checkbox" checked={p.hasPermission}>
                </label>
            {/each}
        </div>
    {/each}
</div>

<style>
    .permission-box {
        display: flex;
    }
</style>