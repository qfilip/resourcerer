<script lang="ts">
    import Notifications from "./Notifications.svelte";


</script>

<header>
    <h1>
        Resourcerer
    </h1>
    <div>
        <Notifications />
    </div>
    <button>Login</button>
</header>

<style>
    header {
        background-color: var(--color-black);
        position: sticky;
        top: 0;
        display: flex;
        align-items: center;
        justify-content: space-between;
    }

    h1 {
        margin: 0;
        padding: var(--padding);
        color: var(--color-white);
    }

    button {
        padding: var(--padding);
    }
    
</style>