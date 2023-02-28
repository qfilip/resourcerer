<script lang="ts">
    import { onMount } from "svelte";
    import * as elementController from "../../controllers/element.controller";
    import type IElementTableDetails from "../../interfaces/appModels/IElementTableDetails";
    import type IElementPurchasedEvent from "../../interfaces/dbModels/IElementPurchasedEvent";


    onMount(() => {
        elementController.getElementsForTable().then(data => {
            
        });
    });

    let elements: IElementTableDetails[] = [];
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