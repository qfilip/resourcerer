<script lang="ts">
    import { onMount } from 'svelte';

    import { ePermissionSection } from '../../interfaces/dtos/enums';
    import { onUserChanged } from '../../stores/user.store';
    import { changePage } from '../../stores/commonUi/page.service';
    import type { PageName } from '../../stores/commonUi/page.service';
    
    type NavButton = { pageName: PageName, permissionSection: string, icon: string };
    let expanded = false;

    onMount(() => {
        onUserChanged(user => {
            visibleButtons = allButtons.filter(x => {
                if(x.permissionSection === null) return true;
                return user.permissions[x.permissionSection];
            });
        });
    });

    const allButtons: NavButton[] = [
        { 
            pageName: 'Categories',
            permissionSection: ePermissionSection[ePermissionSection.Category],
            icon: 'las la-clipboard-list'
        },
        {
            pageName: 'Elements',
            permissionSection: ePermissionSection[ePermissionSection.Element],
            icon: 'las la-vial',
        },
        {
            pageName: 'Settings',
            permissionSection: null,
            icon: 'las la-vial',
        },
        {
            pageName: 'Users',
            permissionSection: ePermissionSection[ePermissionSection.User],
            icon: 'las la-users',
        }
        // { text: 'Composites', icon: 'las la-cubes' },
        // { text: 'Stocks', icon: 'las la-warehouse' },
        // { text: 'Settings', icon: 'las la-wrench' },
    ];

    let visibleButtons: NavButton[] = [];
    let selectedBtnText = visibleButtons.length > 0 ? visibleButtons[0].pageName : '';

    function changeComponent(btn: NavButton) {
        selectedBtnText = btn.pageName;
        changePage(btn.pageName)
    }

    function toggleView() {
        expanded = !expanded;
    }
</script>

<nav class="{expanded ? 'expanded' : 'collapsed'}">
    {#each visibleButtons as btn}
        <button on:click={() => changeComponent(btn)} class="{selectedBtnText === btn.pageName ? 'marked' : ''}">
            <span>{btn.pageName}</span>
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