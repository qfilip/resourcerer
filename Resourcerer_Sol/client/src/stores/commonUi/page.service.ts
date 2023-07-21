import Login from '../../components/account/Login.svelte';
import Register from '../../components/account/Register.svelte';
import { writable } from "svelte/store";
import Settings from '../../components/account/Settings.svelte';


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
    minPermission: { section: string; level: number }
    props: unknown;
}

const pages: IAppPage[] = [
    { name: '', component: Login, hasNav: false, minPermission: null, props: null },
    { name: 'Login', component: Login, hasNav: false, minPermission: null, props: null },
    { name: 'Register', component: Register, hasNav: false, minPermission: null, props: null },
    { name: 'Settings', component: Settings, hasNav: true, minPermission: null, props: null },
];

const currentPage$ = writable<IAppPage>(pages[0]);

export function changePage(name: PageName, props?: unknown) {
    const page = pages.find(x => x.name === name);
    page.props = props;
    currentPage$.set(page);
}

export const onCurrentPageChanged = currentPage$.subscribe;