import axios from 'axios';
import * as loaderService from '../services/commonUi/loader.service';
import * as userStore from '../services/user.store';

const apiUrl = 'https://localhost:44387/api/1.0';
let interceptor;

userStore.jwt(x => {
  if(interceptor) {
    axios.interceptors.request.eject(interceptor);
  }

  interceptor = axios.interceptors.request.use(
    (config) => {
        loaderService.show();
        config.url = apiUrl + config.url;
        config.headers.Authorization = `Bearer ${x}`;
        return config;
    },
    (error) => {
        console.warn(error);
        return Promise.reject(error);
    }
  );
});

axios.interceptors.response.use((response) => {
    loaderService.hide();
    return response;
  }, (error) => {
    loaderService.hide();
    console.warn(error);
    return Promise.reject(error);
  });

export const http = axios;