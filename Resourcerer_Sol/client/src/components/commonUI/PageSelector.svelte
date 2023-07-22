<script lang="ts">
    import { onMount } from 'svelte';
    import * as pageService from '../../stores/commonUi/page.store';
    
    onMount(() => {
        pageService.onCurrentPageChanged(x => {
            console.log('page changed')
            selected = x;
        });
    });

    let selected;
</script>

<section>
    {#if selected}
        <svelte:component this={selected.component} />
    {/if}
</section>

<style>
    
</style>