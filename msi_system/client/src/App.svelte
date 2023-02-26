<script lang="ts">
    import type ICategory from "./interfaces/ICategory";
    import type IComposite from "./interfaces/IComposite";
    import type IElement from "./interfaces/IElement";
    import type IEntityBase from "./interfaces/IEntityBase";
    import type IExcerpt from "./interfaces/IExcerpt";

    function makeEntity<T extends IEntityBase>(retn: () => T) {
        const now = new Date();
        let t = retn();

        t.id = Math.random().toString(16).substring(2);
        t.createdAt = now;
        t.modifiedAt = now;

        return t;
    }

    function mockData() {
        let elements: IElement[] = [];
        let excerpts: IExcerpt[] = [];
        let composites: IComposite[] = [];

        // const spiritsCatg = makeEntity<ICategory>(() => {
        //     return { name: 'Spirits' } as ICategory;
        // });

        // const alesCatg = makeEntity<ICategory>(() => {
        //     return { name: 'Ales' } as ICategory;
        // });

        // const watersCatg = makeEntity<ICategory>(() => {
        //     return { name: 'Waters' } as ICategory;
        // });

        // const veggiesCatg = makeEntity<ICategory>(() => {
        //     return { name: 'Veggies' } as ICategory;
        // });

        const mkElement = (name: string) => makeEntity<IElement>(() => {
            return { name: name } as IElement;
        });

        const mkComposite = (name: string) => makeEntity<IComposite>(() => {
            return { name: name } as IComposite;
        });

        const mkExcerpt = (compName: string, elName: string) => makeEntity<IExcerpt>(() => {
            return {
                elementId: elements.find(x => x.name === elName).id,
                compositeId: composites.find(x => x.name === compName).id
            } as IExcerpt;
        });

        ['vodka', 'ginger ale', 'lime', 'rum', 'gin', 'sparkling water'].forEach(x => elements.push(mkElement(x)));
        ['moscow mule', 'dark n stormy', 'gin fizz'].forEach(x => composites.push(mkComposite(x)));
        [
            {compName: 'moscow mule', ings: ['vodka', 'ginger ale', 'lime']},
            {compName: 'dark n stormy', ings: ['rum', 'ginger ale', 'lime']},
            {compName: 'gin fizz', ings: ['gin', 'sparkling water', 'lime']},
            
        ].forEach(x => x.els.forEach(el => excerpts.push(mkExcerpt(x.c, el))));
    }

    // moscow mule      {vodka, ginger ale, lime}
    // dark n stormy    {rum, ginger ale, lime}
    // gin fizz         {gin, sparkling water, lime}

</script>

<main>
    <button on:click={mockData}>Mock</button>
</main>

<style>
	
</style>