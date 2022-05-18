import classes from './NavigationLayout.module.css';

import netflixLogoUrl from '../../netflix-logo.svg';
import { Outlet } from 'react-router-dom';

const NavigationLayout = (props) => {
    return (
        <div className={classes['full-size-page']}>
            <nav className={classes['navbar']}>
                <ul className={classes['navbar-left-list']}>
                    <li>
                        <a href="/">Фільми</a>
                    </li>
                </ul>
                <a className={classes['navbar-logo-container']} href="/">
                    <img src={netflixLogoUrl} />
                </a>
                <ul className={classes['navbar-right-list']}>
                    <li>
                        <a href="/">Замовити снеки</a>
                    </li>
                    <li>
                        <a href="/">Контакти</a>
                    </li>
                </ul>
            </nav>

            <Outlet />
        </div>
    );
};

export default NavigationLayout;
