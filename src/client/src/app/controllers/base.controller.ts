import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class BaseController {
    protected apiUrl = 'http://localhost:24822';
    protected http = inject(HttpClient);
}