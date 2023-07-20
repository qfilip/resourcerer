<script lang="ts">
    import { onMount } from "svelte";
    import * as categoryService from "../../services/category.service";
    import type { ICategoryDto } from "../../interfaces/dtos/interfaces";

    export let category: ICategoryDto;
    export let allCategories: ICategoryDto[];

    onMount(() => {
        categoryService.selectedCategoryId$.subscribe(x => {
            selectedCategoryId = x;
        });
    });


    let expanded = true;
    let selectedCategoryId = '';
    let childCategories = allCategories.filter(x => x.parentCategoryId === category.id);

    function toggleExpandAndSelect() {
        expanded = !expanded;
        categoryService.selectedCategoryId$.set(category.id);
    }
</script>

<div>
    <button on:click={toggleExpandAndSelect} class="{category.id === selectedCategoryId ? 'marked' : ''} dd-button">
        {#if !expanded}
            <i class="las la-angle-right"></i>
        {:else}
            <i class="las la-angle-down"></i>
        {/if}

        {category.name}
    </button>
    <div class="{expanded ? 'expanded' : 'collapsed'} subcategory">
        {#each childCategories as child}
            <svelte:self category={child} allCategories={allCategories} />
        {/each}
    </div>
</div>


<style>
    .dd-button {
        font-size: 1rem;
        margin: .2rem .2rem;
        padding: 0 .2rem;
        text-align: start;
        border: none;
        transition: none;
        color: var(--color-black);
        background-color: transparent;
        width: fit-content;
    }

    .marked {
        color: var(--color-white);
        background-color: var(--color-red);
    }

    .subcategory {
        margin-left: 1rem;
    }

    .expanded {
        height: auto;
    }

    .collapsed {
        height: 0;
        visibility: hidden;
    }
</style>