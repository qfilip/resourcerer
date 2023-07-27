<script lang="ts">
    import { onMount } from "svelte";
    import * as controller from "../../controllers/user.controller";
    import type { IAppUserDto } from "../../interfaces/dtos/interfaces";
    
    onMount(() => {
        controller.getAll().then(xs => users = xs as IAppUserDto[]);
    })

    let users: IAppUserDto[] = [];
</script>

<h2>Users</h2>
<section>
    <div>
        <h3>Names</h3>
        {#each users as u}
            <div>{u.name}</div>
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
</style>