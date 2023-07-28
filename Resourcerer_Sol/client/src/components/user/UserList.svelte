<script lang="ts">
    import { onMount } from "svelte";
    import * as controller from "../../controllers/user.controller";
    import type { IAppUserDto } from "../../interfaces/dtos/interfaces";
    
    onMount(() => {
        controller.getAll().then(xs => users = xs as IAppUserDto[]);
    })

    let users: IAppUserDto[] = [];
    let selectedUserName: string;

    function selectUser(userId: string) {
        selectedUserName = users.find(x => x.id === userId).name;
        if(!selectedUserName) return;

        controller.getUser(userId).then(x => console.log(x));
    }
</script>

<h2>Users</h2>
<section>
    <div>
        <h3>Names</h3>
        {#each users as u}
            <button
                on:click={() => selectUser(u.id)}
                class="user-select-button"
                class:selected={selectedUserName === u.name}>
                {u.name}
            </button>
        {/each}
    </div>
    <div>
        <h3>Details</h3>
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