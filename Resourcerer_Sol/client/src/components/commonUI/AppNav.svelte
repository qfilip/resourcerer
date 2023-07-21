<script lang="ts">
    import { onMount } from 'svelte';

    import { onUserChanged } from '../../stores/user.store';
    import * as pageStore from '../../stores/commonUi/page.service';
    import type { IAppPage } from '../../stores/commonUi/page.service';
    
    let expanded = false;
    let user;

    onMount(() => {
        onUserChanged(user => {
            user = user;
            visibleButtons = pageStore.pages.filter(x => {
                if(x.minPermission === null) return true;
                return Object.keys(user.permissions).includes(x.minPermission);
            });
        });

        pageStore.onCurrentPageChanged(page => selectedBtnText = page.name);
    });

    let visibleButtons: IAppPage[] = [];
    let selectedBtnText = visibleButtons.length > 0 ? visibleButtons[0].name : '';

    function changeComponent(btn: IAppPage) {
        selectedBtnText = btn.name;
        pageStore.changePage(btn.name)
    }

    function toggleView() {
        expanded = !expanded;
    }
</script>

<nav class="{expanded ? 'expanded' : 'collapsed'}">
    {#each visibleButtons as btn}
        <button on:click={() => changeComponent(btn)} class="{selectedBtnText === btn.name ? 'marked' : ''}">
            <span>{btn.name}</span>
            <i class="{btn.icon}"></i>
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