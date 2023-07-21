<script lang="ts">
    import { onMount } from 'svelte';
    import * as pageService from '../../stores/commonUi/page.service';
    import AppNav from './AppNav.svelte';
    
    onMount(() => {
        pageService.onCurrentPageChanged(x => {
            selected = x;
        });
    });

    let selected;
</script>

<section>
    {#if selected}
        {#if selected.hasNav}
            <AppNav />
        {/if}
        <div>
            <svelte:component this={selected.component} {...selected.props }/>
        </div>
    {/if}
</section>

<style>
    section {
        display: flex;
    }

    div {
        width: 100%;
    }
</style>