<script lang="ts">
    import { onMount } from "svelte";
    import AppHeader from "./components/commonUI/AppHeader.svelte";
    import PageLoader from "./components/commonUI/PageLoader.svelte";
    import PageSelector from "./components/commonUI/PageSelector.svelte";
    import { jwtChangedEvent } from "./stores/user.store";
    import { setInterceptor } from "./controllers/base.controller";
    import { wakeUp } from "./stores/commonUi/sleep.store";

    onMount(() => {
        jwtChangedEvent(x => setInterceptor(x));
    });

    function onClicked() {
        wakeUp();
    }
</script>

<div on:click={onClicked} on:keyup={onClicked}>
    <PageLoader />
    <AppHeader />
    <main>
        <PageSelector />
    </main>
    <!-- <AppTest /> -->
</div>

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

    :global(.b-blue) {
        background-color: var(--color-blue);
    }

    :global(.b-orange) {
        background-color: var(--color-orange);
    }

    :global(.b-red) {
        background-color: var(--color-red);
    }
</style>