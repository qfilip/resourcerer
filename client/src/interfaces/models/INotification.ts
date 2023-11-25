import type { eSeverity } from "../enums/eSeverity";

export interface INotification {
    text: string;
    severity: eSeverity;
}