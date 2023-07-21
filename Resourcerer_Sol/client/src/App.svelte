<script lang="ts">
    import AppTest from "./AppTest.svelte";
    import AppHeader from "./components/commonUI/AppHeader.svelte";
    import Notifications from "./components/commonUI/Notifications.svelte";
    import PageLoader from "./components/commonUI/PageLoader.svelte";
    import PageSelector from "./components/commonUI/PageSelector.svelte";
</script>

<PageLoader />
<AppHeader />
<Notifications />
<main>
    <!-- <AppTest /> -->
    <PageSelector />
</main>

<style>
	:root {
        --padding: .5rem;
        --padding-sm: .2rem;

        --color-white: whitesmoke;
        --color-lightgrey: #ddd;
        --color-black: #222;
        --color-orange: #ff6200;
        --color-red: #cc3333;
        --color-blue: #007bff;
        
        --transition: .5s;
    }

    :global(::selection) {
        color: var(--color-white);
        background: var(--color-red);
    }

    :global(html, body) {
        margin: 0;
        padding: 0;
        background-color: var(--color-lightgrey);
    }

    :global(button) {
        width: 5rem;
        padding: 0;
        margin: 0;
        border: .15rem ridge var(--color-red);
        border-radius: 0;
        transition: var(--transition);
        cursor: pointer;
    }

    :global(button:hover) {
        color: var(--color-white);
        background-color: var(--color-black);
    }

    :global(input) {
        padding: var(--padding-sm);
        transition: var(--transition);
    }

    :global(input:focus) {
        color: var(--color-white);
        background-color: var(--color-black);
    }

    :global(.info) {
        background-color: var(--color-blue);
    }

    :global(.warning) {
        background-color: var(--color-orange);
    }

    :global(.error) {
        background-color: var(--color-red);
    }
</style>