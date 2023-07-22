<script lang="ts">
    import Profile from "../account/Profile.svelte";
import Notifications from "./Notifications.svelte";


</script>

<header>
    <h1>
        Resourcerer
    </h1>
    <Notifications />
    <Profile />
</header>

<style>
    header {
        background-color: var(--color-black);
        position: sticky;
        top: 0;
        padding: var(--padding);
        display: flex;
        align-items: center;
        justify-content: space-between;
    }

    h1 {
        margin: 0;
        color: var(--color-white);
    }
    
</style>