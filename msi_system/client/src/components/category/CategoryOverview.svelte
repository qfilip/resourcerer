<script lang="ts">
    import { onMount } from 'svelte';
    import CategoryDetails from './CategoryDetails.svelte';
    import CategoryDropdown from './CategoryDropdown.svelte';
    import * as categoryService from "../../services/category.service";
    import * as categoryController from "../../controllers/category.controller";
    import type ICategory from '../../interfaces/dbModels/ICategory';

    onMount(() => {
        categoryController.getCategories().then(x => {
            categories = x;
            categoryService.selectedCategoryId$.set(x[0].id);
        });

        categoryService.selectedCategoryId$.subscribe(x => {
            selectedCategory = categories.find(c => c.id === x);
        });
    });
    let categories: ICategory[] = [];
    let selectedCategory;
    $: mainCategories = categories.filter(x => !x.parentCategoryId);
</script>

<h2>
    Categories
</h2>

<section>
    <div>
        {#each mainCategories as ctg}
            <CategoryDropdown category={ctg} allCategories={categories} />
        {/each}
    </div>
    <div>
        <CategoryDetails category={selectedCategory} />
    </div>
</section>

<style>
    h2 {
        text-align: center;
    }

    section {
        margin: 1rem;
        
        display: flex;
        justify-content: center;
        align-items: center;
    }

    section > div {
        margin: 2rem;
        width: 30%;
        border: .1rem groove lightgrey;
        min-height: 50vh;
        overflow-y: auto;
    }
</style>