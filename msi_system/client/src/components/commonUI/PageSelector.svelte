<script lang="ts">
    import { onMount } from 'svelte';
    import * as pageService from '../../services/commonUi/page.service';
    
    onMount(() => {
        pageService.onCurrentPageChanged(x => selected = x);
    });

    let selected;
    
</script>

{#if selected}
    <svelte:component this={selected.component} {...selected.props }/>
{/if}

<style>

</style>