<script lang="ts">
    import { onMount } from "svelte";
    import { logout, userChangedEvent } from "../../stores/user.store";
    import type { IAppUserDto } from "../../interfaces/dtos/interfaces";

    onMount(() => {
        userChangedEvent(x => user = x);
    });

    let user:IAppUserDto;
</script>

<div>
    {#if user}
    <div class="flex">
        <div>{user.name}</div>
        <button on:click={logout}>
            <i class="las la-lock"></i>
        </button>
    </div>
    {/if}
</div>

<style>
    .flex {
        display: flex;
        align-items: center;
        justify-content: space-between;
        color: white;
    }

    button {
        border: none;
        background-color: inherit;
        transition: .3s;
        width: 3rem;
        height: 3rem;
    }

    i {
        color: var(--color-white);
        font-size: 2rem;
        transition: .3s;
    }

    i:hover {
        color: var(--color-red);
    }
</style>
