<script lang="ts">
    import { setPermissions } from "../../controllers/user.controller";
    import { getEnumKeys } from "../../functions/enums";
    import { ePermission, ePermissionSection } from "../../interfaces/dtos/enums";
    import type { IAppUserDto, ISetUserPermissionsDto } from "../../interfaces/dtos/interfaces";
    import { createEventDispatcher } from 'svelte';

    const dispatch = createEventDispatcher();

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

    function updatePermissions() {
        let newPermissions: { [key: string]: number } = {}
        
        allPermissions.forEach(x => {
            const permissionLevel: number = x.permissions
                .filter(p => p.hasPermission)
                .map(p => ePermission[p.permission])
                .reduce((acc, x) => acc | x, 0);

            newPermissions[x.section] = permissionLevel;
        });

        const dto: ISetUserPermissionsDto = {
            userId: user.id,
            permissions: newPermissions
        };

        setPermissions(dto).then(x => {
            dispatch('permissionsUpdated', x as IAppUserDto);
        });
    }

    function togglePermission(p: { permission: string, hasPermission: boolean}) {
        p.hasPermission = !p.hasPermission;
    }

    $: {
        if(user) {
            allPermissions = getAllPermissions();
            const sections = getEnumKeys(ePermissionSection);
            const permissions = getEnumKeys(ePermission);
            
            sections.forEach(s => {
                const permissionLevel = user.permissions[s];
                if(!permissionLevel) return;
                
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

<button on:click={updatePermissions}>Set permissions</button>
<div>
    {#each allPermissions as ps}
        <div>
            {ps.section}
        </div>
        <div class="permission-box">
            {#each ps.permissions as p}
                <label>
                    <span>{p.permission} &nbsp;</span>
                    <input type="checkbox" checked={p.hasPermission} on:click={() => togglePermission(p)}>
                </label>
            {/each}
        </div>
    {/each}
</div>

<style>
    button {
        width: 100%;
    }

    .permission-box {
        display: flex;
    }

    label {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin: .5rem;
    }

    label > input {
        margin: 0;
    }
</style>