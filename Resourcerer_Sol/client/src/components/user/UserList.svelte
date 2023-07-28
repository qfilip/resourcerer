<script lang="ts">
    import { onMount } from "svelte";
    import * as controller from "../../controllers/user.controller";
    import type { IAppUserDto } from "../../interfaces/dtos/interfaces";
    import UserPermissions from "./UserPermissions.svelte";
    
    onMount(() => {
        controller.getAll().then(xs => users = xs as IAppUserDto[]);
    })

    let users: IAppUserDto[] = [];
    let selectedUser: IAppUserDto = null;

    function selectUser(userId: string) {
        selectedUser = users.find(x => x.id === userId);
        if(!selectedUser) return;

        controller.getUser(userId).then(x => console.log(x));
    }
</script>

<h2>Users</h2>
<section>
    <div class="name-list">
        <h3>Names</h3>
        {#each users as u}
            <button
                on:click={() => selectUser(u.id)}
                class="user-select-button"
                class:selected={selectedUser && selectedUser.name === u.name}>
                {u.name}
            </button>
        {/each}
    </div>
    <div>
        <h3>Details</h3>
        <div>
            Name:
            {#if selectedUser}
                <span>{selectedUser.name}</span>
            {/if}
        </div>
        <div>
            Status:
            {#if selectedUser}
                <span>{selectedUser.entityStatus}</span>
            {/if}
        </div>
        <div>
            Permissions:
            {#if selectedUser !== null}
                <UserPermissions user={selectedUser}/>
            {/if}
        </div>
    </div>
</section>

<style>
    h2 {
        text-align: center;
    }

    section {
        margin: 1rem;
        
        display: flex;
        justify-content: space-evenly;
        align-items: center;
    }

    section > div {
        min-height: 50vh;
        overflow-y: auto;
    }

    .name-list {
        width: 20%;
    }

    .user-select-button {
        display: block;
        outline: none;
        border: none;
        background-color: transparent;
        transition: .3s;
    }

    .user-select-button:hover {
        color: var(--color-red);
    }

    .selected {
        color: var(--color-white);
        background-color: var(--color-red);
    }
</style>