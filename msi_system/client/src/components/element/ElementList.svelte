<script lang="ts">
    import { onMount } from "svelte";
    import * as elementController from "../../controllers/element.controller";
    import type IElement from "../../interfaces/dbModels/IElement";

    onMount(() => {
        elementController.getElementsForTable().then(x => {
            console.log(x)
            // elements = x;
        });
    });

    let elements: IElement[] = [];
</script>

<h2>
    Elements
</h2>
<div>
    <table>
        <thead>
            <tr>
                <th>Name</th>
                <th>Category</th>
                <th>In stock</th>
              </tr>
        </thead>
        <tbody>
            {#each elements as elem}
                <tr>
                    <td>Ernst Handel</td>
                    <td>Roland Mendel</td>
                    <td>Austria</td>
                </tr>
            {/each}
        </tbody>
    </table>
</div>