import type { HomeComponentName } from "../types/HomeComponentName";

export interface IHomeComponent {
    name: HomeComponentName;
    component: any;
    minPermission: string;
    icon: string;
}