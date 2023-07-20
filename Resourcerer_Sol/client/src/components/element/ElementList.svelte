<script lang="ts">
    import { onMount } from "svelte";
    import * as elementController from "../../controllers/element.controller";

    onMount(() => {
        elementController.getElementsOverviews().then((data) => {
            
        });
    });

</script>

<h2>Elements</h2>
<section>
    <table>
        <thead>
            <tr>
                <th>Name</th>
                <th>Category</th>
                <th>Unit of Measure</th>
                <th>In composites</th>
            </tr>
        </thead>
        <tbody>
            <!-- {#each elements as elem}
                <tr on:click={() => console.log(elem.id)}>
                    <td>{elem.name}</td>
                    <td>{elem.category}</td>
                    <td>{elem.unitOfMeasure}</td>
                    <td>{elem.compositesCount}</td>
                    <div>aaaaa</div>
                </tr>
            {/each} -->
        </tbody>
    </table>
</section>

<style>
    h2 {
        text-align: center;
    }
    section {
        padding: var(--padding);
    }
    table {
        table-layout: fixed;
        width: 50%;
        border: .1rem solid var(--color-black);
        border-collapse: collapse;
    }

    thead th:nth-child(1) {
        width: 30%;
    }

    thead th:nth-child(2) {
        width: 20%;
    }

    thead th:nth-child(3) {
        width: 25%;
    }

    thead th:nth-child(4) {
        width: 25%;
    }

    tbody > tr {
        position: relative;
    }

    tbody > tr > div {
        position: absolute;
    }

    tr {
        border-bottom: .1rem solid var(--color-black) !important;
    }

    th, td {
        text-align: center;
    }
</style>
