import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';

import NavigationLayout from './layouts/NavigationLayout/NavigationLayout';
import Films from './pages/Films/Films';
import Film from './pages/Film/Film';
import Booking from './pages/Booking/Booking';

import { ThemeProvider, createTheme } from '@mui/material/styles';

import 'normalize.css';
import './index.css';

const darkTheme = createTheme({
    palette: {
        mode: 'dark',
        primary: {
            main: '#e21e20',
        },
        secondary: {
            main: '#ffffff',
        },
    },
});

window.apiBaseUrl = 'https://localhost:58273/api';

ReactDOM.render(
    <React.StrictMode>
        <ThemeProvider theme={darkTheme}>
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<NavigationLayout />}>
                        <Route index element={<Navigate replace to="/films" />} />
                        <Route path="films">
                            <Route index element={<Films />} />
                            <Route path=":filmId">
                                <Route index element={<Film />} />
                                <Route path="book" element={<Booking />}></Route>
                            </Route>
                        </Route>
                    </Route>
                    <Route
                        path="*"
                        element={
                            <main style={{ padding: '1rem' }}>
                                <p>404! There's nothing here!</p>
                            </main>
                        }
                    />
                </Routes>
            </BrowserRouter>
        </ThemeProvider>
    </React.StrictMode>,
    document.getElementById('root')
);
