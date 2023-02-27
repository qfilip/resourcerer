<script lang="ts">
    import { createEventDispatcher } from 'svelte';

    const dispatch = createEventDispatcher();
    let expanded = true;
    
    const buttons = [
        { text: 'Categories', icon: 'las la-clipboard-list' },
        { text: 'Elements', icon: 'las la-vial' },
        { text: 'Composites', icon: 'las la-cubes' },
        { text: 'Stocks', icon: 'las la-warehouse' },
        { text: 'Settings', icon: 'las la-wrench' },
        { text: 'Users', icon: 'las la-users' }
    ]

    let selectedBtnText = buttons[0].text;

    function changeComponent(btnText: string) {
        selectedBtnText = btnText;
        dispatch('componentSelected', {
			name: btnText
		});
    }

    function toggleView() {
        expanded = !expanded;
    }
</script>

<nav class="{expanded ? 'expanded' : 'collapsed'}">
    {#each buttons as btn}
        <button on:click={() => changeComponent(btn.text)} class="{selectedBtnText === btn.text ? 'marked' : ''}">
            <span>{btn.text}</span>
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