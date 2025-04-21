import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable, catchError, of, filter, tap } from "rxjs";
import { LoaderService } from "../features/common-ui/services/loader.service";
import { PopupService } from "../features/common-ui/services/popup.service";

@Injectable({
    providedIn: 'root'
})
export class BaseApiService {
    protected apiUrl = 'http://localhost:24822/v1.0';
    
    protected http = inject(HttpClient);
    protected popup = inject(PopupService);
    protected loader = inject(LoaderService);

    protected withLoader = <T>(alwaysHide = false) => (source: Observable<T>): Observable<T> => {
        this.loader.show();
        return source.pipe(
            catchError(err => {
                if(err instanceof HttpErrorResponse) {
                    const e = err as HttpErrorResponse;
                    this.popup.error(e.statusText);
                }
                this.loader.hide();
                return of(null);
            }),
            tap(x => {
                if (x) this.loader.hide();
                if(alwaysHide) this.loader.hide();
            }),
            filter(x => x !== null)
        );
    }
}