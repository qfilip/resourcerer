<script lang="ts">
    import { onMount } from 'svelte';
    import * as mock from '../../utils'
    import CategoryDetails from './CategoryDetails.svelte';
    import CategoryDropdown from './CategoryDropdown.svelte';
    import * as categoryService from "../../services/category.service";
    
    let db = mock.seedDbMock();

    onMount(() => {
        categoryService.selectedCategoryId$.subscribe(x => {
            selectedCategory = db.categories.find(c => c.id === x);
        });
    });

    let selectedCategory;
    let mainCategories = db.categories.filter(x => !x.parentCategoryId);
</script>

<h2>
    Categories
</h2>

<section>
    <div>
        {#each mainCategories as ctg}
            <CategoryDropdown category={ctg} allCategories={db.categories} />
        {/each}
    </div>
    <div>
        <CategoryDetails />
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
        min-height: 30vh;
    }
</style>