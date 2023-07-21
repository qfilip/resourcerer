<script lang="ts">
    import { onMount } from 'svelte';
    import CategoryDropdown from './CategoryDropdown.svelte';
    import * as categoryService from "../../stores/category.service";
    import * as categoryController from "../../controllers/category.controller";
    import type { ICategoryDto } from '../../interfaces/dtos/interfaces';

    onMount(() => {
        categoryController.getAllCategories()
            .then(x => {
                console.log(x);
                categories = x;
            });

        categoryService.selectedCategoryId$.subscribe(x => {
            selectedCategory = categories.find(c => c.id === x);
        });
    });
    let categories: ICategoryDto[] = [];
    let selectedCategory;
    $: mainCategories = categories.filter(x => !x.parentCategoryId);
</script>

<h2>
    Categories
</h2>

<section>
    <div class="dropdowns">
        <h3>Tree</h3>
        {#each mainCategories as ctg}
            <CategoryDropdown category={ctg} allCategories={categories} />
        {/each}
    </div>
    <div class="actions">
        <h3>Actions</h3>
        <button>
            <i class="las la-plus"></i>
        </button>
        <button>
            <i class="las la-chart-line"></i>
        </button>
        <button>
            <i class="las la-trash"></i>
        </button>
    </div>
    <div>
        <h3>Info</h3>
        <div>
            Composites: 0
        </div>
        <div>
            Elements: 0
        </div>
    </div>
</section>

<style>
    h2, h3 {
        text-align: center;
    }

    section {
        margin: 1rem;
        
        display: flex;
        justify-content: space-evenly;
        align-items: center;
    }

    section > div {
        min-height: 50vh;
        overflow-y: auto;
    }

    .dropdowns {
        margin: 2rem;
    }

    .actions > button {
        font-size: 4rem;
        height: 7rem;
        width: 7rem;
        color: var(--color-black);
        background-color: transparent;
    }

    .actions > button:hover {
        color: var(--color-red);
        background-color: var(--color-black);
    }
</style>