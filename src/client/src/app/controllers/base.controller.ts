import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class BaseController {
    protected apiUrl = 'http://localhost:24822/api/1.0';
    protected http = inject(HttpClient);
}