<script lang="ts">
    import { onMount } from 'svelte';

    import { onUserChanged } from '../../stores/user.store';
    import * as homenavStore from '../../stores/home/homenav.store';
    import type { IHomeComponent } from '../../interfaces/models/IHomeComponent';
    
    let expanded = false;

    onMount(() => {
        onUserChanged(user => {
            visibleItems = homenavStore.components.filter(x => {
                if(x.minPermission === null) return true;
                return Object.keys(user.permissions).includes(x.minPermission);
            });
        });

        homenavStore.currentComponentChanged(page => selectedItemText = page.name);
    });

    let visibleItems: IHomeComponent[] = [];
    let selectedItemText = visibleItems.length > 0 ? visibleItems[0].name : '';

    function changeToComponent(hc: IHomeComponent) {
        selectedItemText = hc.name;
        homenavStore.setComponent(hc.name);
    }

    function toggleView() {
        expanded = !expanded;
    }
</script>

<nav class="{expanded ? 'expanded' : 'collapsed'}">
    {#each visibleItems as item}
        <button on:click={() => changeToComponent(item)} class="{selectedItemText === item.name ? 'marked' : ''}">
            <span>{item.name}</span>
            <i class="{item.icon}"></i>
        </button>
    {/each}

    <button on:click={toggleView} class="expander">
        {#if expanded}
            <i class="las la-angle-left"></i>
        {:else}
            <i class="las la-angle-right"></i>
        {/if}
    </button>
</nav>

<style>
    nav {
        height: 80vh;
        overflow-y: hidden;
        color: var(--color-white);
        background-color: var(--color-black);
        transition: var(--transition);
    }

    nav > button {
        padding: .5rem .2rem;
        border: none;
        width: 100%;
        color: var(--color-white);
        background-color: var(--color-black);

        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    nav > button:hover {
        background-color: var(--color-red);
    }

    button, i {
        font-size: 1rem;
    }

    .marked {
        background-color: var(--color-red);
    }

    .expander {
        justify-content: center !important;
    }

    .expanded {
        width: 10%;
    }

    .collapsed {
        width: 3%;
    }

    .expanded > button {
        justify-content: space-between;
    }

    .collapsed > button {
        justify-content: center;
    }

    .expanded > button > span {
        display: inline;
    }

    .collapsed > button > span {
        display: none;
    }

    .expanded > button > i {
        font-size: 1rem;
    }

    .collapsed > button > i {
        font-size: 1.2rem;
    }
</style>