<script lang="ts">
    import HomeNav from "./HomeNav.svelte";
    import CategoryOverview from '../category/CategoryOverview.svelte';

    const components = [
        { name: 'Categories', component: CategoryOverview },
        { name: 'Elements', icon: 'las la-vial', component: null },
        { name: 'Composites', icon: 'las la-cubes', component: null },
        { name: 'Stocks', icon: 'las la-warehouse', component: null },
        { name: 'Settings', icon: 'las la-wrench', component: null },
        { name: 'Users', icon: 'las la-users', component: null },
    ];

    let selectedComponent = components[0].component;

    function onComponentSelected(ev: any) {
        console.log(ev.detail);
        selectedComponent = components.find(x => ev.detail.name === x.name).component;
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