import type { IHomeComponent } from "../../interfaces/models/IHomeComponent";
import { ePermissionSection } from '../../interfaces/dtos/enums';

import Settings from '../../components/account/Settings.svelte';
import CategoryOverview from '../../components/category/CategoryOverview.svelte';
import ElementList from '../../components/element/ElementList.svelte';
import { writable } from "svelte/store";
import type { HomeComponentName } from "../../interfaces/types/HomeComponentName";

export const components: IHomeComponent[] = [
    { 
        name: 'Categories',
        component: CategoryOverview,
        minPermission: ePermissionSection[ePermissionSection.Category],
        icon: 'las la-clipboard-list'
    },
    { 
        name: 'Elements',
        component: ElementList,
        minPermission: ePermissionSection[ePermissionSection.Element],
        icon: 'las la-vial'
    },
    // { 
    //     name: 'Users',
    //     component: Register,
    //     minPermission: ePermissionSection[ePermissionSection.User],
    //     icon: 'las la-users'
    // },
    { 
        name: 'Settings',
        component: Settings,
        minPermission: null,
        icon: 'las la-wrench'
    }
];

const currentComponent$ = writable<IHomeComponent>(components[0]);

export function setComponent(name: HomeComponentName) {
    const component = components.find(x => x.name === name);
    currentComponent$.set(component);
}

export const currentComponentChanged = currentComponent$.subscribe;

export const goto = {
    categories: function() {
        setComponent('Categories');
    },
    elements: function() {
        setComponent('Elements');
    },
    settings: function() {
        setComponent('Settings');
    }
}