<script lang="ts">
    import { onMount } from 'svelte';

    import CategoryOverview from '../category/CategoryOverview.svelte';
    import ElementList from "../element/ElementList.svelte";
    import type { IAppComponent } from '../../interfaces/models/IAppComponent';

    import { ePermissionSection } from '../../interfaces/dtos/enums';
    import { createEventDispatcher } from 'svelte';
    import { onUserChanged } from '../../stores/user.store';
    
    const dispatch = createEventDispatcher();
    let expanded = false;

    onMount(() => {
        onUserChanged(userr => {
            visibleButtons = allButtons.filter(x => {
                if(x.permissionSection === null) return true;
                return userr.permissions[x.permissionSection];
            });
        });
    });

    

    const allButtons: IAppComponent[] = [
        { 
            name: 'Categories',
            component: CategoryOverview,
            permissionSection: ePermissionSection[ePermissionSection.Category],
            icon: 'las la-clipboard-list'
        },
        {
            name: 'Elements',
            component: ElementList,
            permissionSection: ePermissionSection[ePermissionSection.Element],
            icon: 'las la-vial',
        },
        {
            name: 'Test',
            component: ElementList,
            permissionSection: null,
            icon: 'las la-vial',
        }
        // { text: 'Composites', icon: 'las la-cubes' },
        // { text: 'Stocks', icon: 'las la-warehouse' },
        // { text: 'Settings', icon: 'las la-wrench' },
        // { text: 'Users', icon: 'las la-users' }
    ];

    let visibleButtons: IAppComponent[] = [];

    // onUserChanged(user => {
    //     debugger
    //     visibleButtons = allButtons.filter(x => {
    //         if(x.permissionSection === null) return true;
    //         return user.permissions[x.permissionSection];
    //     });
    // });

    let selectedBtnText = visibleButtons.length > 0 ? visibleButtons[0].name : '';

    function changeComponent(btn: IAppComponent) {
        selectedBtnText = btn.name;
        dispatch('componentSelected', {
			name: btn
		});
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