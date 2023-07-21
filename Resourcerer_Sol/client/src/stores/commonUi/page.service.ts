import Home from '../../components/home/Home.svelte';
import Login from '../../components/account/Login.svelte';
import Register from '../../components/account/Register.svelte';
import { writable } from "svelte/store";


export type PageName = '' | 'login' | 'register' | 'home' | 'test';

export interface IAppPage {
    name: PageName;
    component: any;
    props: unknown;
}

const pages: IAppPage[] = [
    { name: '', component: Login, props: null },
    { name: 'login', component: Login, props: null },
    { name: 'register', component: Register, props: null },
    { name: 'home', component: Home, props: null },
    { name: 'test', component: Home, props: null }
];

const currentPage$ = writable<IAppPage>(pages[0]);

function changePage(name: PageName, props?: unknown) {
    const page = pages.find(x => x.name === name);
    page.props = props;
    
    currentPage$.set(page);
}

export const onCurrentPageChanged = currentPage$.subscribe;

export const goto = {
    default: function() {
        changePage('');
    },
    login: function() {
        changePage('login');
    },
    register: function() {
        changePage('register');
    },
    home: function() {
        changePage('home');
    },
    test: function(props: unknown) {
        changePage('home', props);
    }
}