import Login from '../../components/account/Login.svelte';
import Register from '../../components/account/Register.svelte';
import { writable } from "svelte/store";
import Settings from '../../components/account/Settings.svelte';
import { ePermissionSection } from '../../interfaces/dtos/enums';
import CategoryOverview from '../../components/category/CategoryOverview.svelte';
import ElementList from '../../components/element/ElementList.svelte';


export type PageName =
    | ''
    | 'Login'
    | 'Register'
    | 'Categories'
    | 'Elements'
    | 'Users'
    | 'Settings';

export interface IAppPage {
    name: PageName;
    component: any;
    hasNav: boolean;
    minPermission: string;
    icon: string;
}

export const pages: IAppPage[] = [
    { 
        name: '',
        component: Login,
        hasNav: false,
        minPermission: null,
        icon: null
    },
    { 
        name: 'Login',
        component: Login,
        hasNav: false,
        minPermission: null,
        icon: null
    },
    { 
        name: 'Register',
        component: Register,
        hasNav: false,
        minPermission: null,
        icon: null
    },
    { 
        name: 'Categories',
        component: CategoryOverview,
        hasNav: true,
        minPermission: ePermissionSection[ePermissionSection.Category],
        icon: 'las la-clipboard-list'
    },
    { 
        name: 'Elements',
        component: ElementList,
        hasNav: true,
        minPermission: ePermissionSection[ePermissionSection.Element],
        icon: 'las la-vial'
    },
    { 
        name: 'Users',
        component: Register,
        hasNav: false,
        minPermission: ePermissionSection[ePermissionSection.User],
        icon: 'las la-users'
    },
    { 
        name: 'Settings',
        component: Settings,
        hasNav: true,
        minPermission: null,
        icon: 'las la-wrench'
    },
];

const currentPage$ = writable<IAppPage>(pages[0]);

export function changePage(name: PageName, props?: unknown) {
    const page = pages.find(x => x.name === name);
    // page.props = props;
    currentPage$.set(page);
}

export const onCurrentPageChanged = currentPage$.subscribe;