<script lang="ts">
    import HomeNav from "./HomeNav.svelte";

    let selectedComponent = null;

    function onComponentSelected(ev: any) {
        console.log(ev);
        // selectedComponent = components.find(x => ev.detail.name === x.name).component;
    }
</script>

<section>
    <HomeNav on:componentSelected={onComponentSelected} />
    <div>
        {#if selectedComponent}
            <svelte:component this={selectedComponent} />
        {/if}
    </div>
</section>

<style>
    section {
        display: flex;
    }

    div {
        width: 100%;
    }
</style>