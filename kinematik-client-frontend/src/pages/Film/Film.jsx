import React, { useEffect } from 'react';
import classes from './Film.module.css';

import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import Button from '../../components/Button/Button';
import Modal from '@mui/material/Modal';
import ReactPlayer from 'react-player/youtube';
import CircularProgress from '@mui/material/CircularProgress';

import genres from '../../domain/genres';

const runtimeMinutesToString = function (runtimeMinutes) {
    const fullHours = Math.floor(runtimeMinutes / 60);
    const minutes = runtimeMinutes % 60;

    let resultTokens = [];

    if (fullHours > 0) {
        resultTokens.push(`${fullHours} год.`);
    }
    if (minutes > 0) {
        resultTokens.push(`${minutes} хв.`);
    }

    return resultTokens.join(' ');
};

const Film = (props) => {
    const params = useParams();

    let [film, setFilm] = useState(null);
    let [filmIsLoaded, setFilmIsLoaded] = useState(false);

    useEffect(async () => {
        const rawResponse = await fetch(
            `${window.apiBaseUrl}/films/${params.filmId}/details`
        );
        const parsedResponse = await rawResponse.json();

        setFilm(parsedResponse);
        setFilmIsLoaded(true);
    }, []);

    film = {
        ...film,
        airLanguage: 'Українська',
        notableStaff: [
            {
                picture: 'https://i.pravatar.cc?img=1',
                title: 'Directed by',
                description: 'Jonathan Liebesman',
            },
            {
                picture: 'https://i.pravatar.cc?img=2',
                title: 'Produced by',
                description: 'Basil Iwanyk',
            },
            {
                picture: 'https://i.pravatar.cc?img=3',
                title: 'Screenplay by',
                description: 'Savid Leslie Johnson',
            },
            {
                picture: 'https://i.pravatar.cc?img=4',
                title: 'Based on',
                description: 'Characters by Beverley Cross',
            },
            {
                picture: 'https://i.pravatar.cc?img=5',
                title: 'Main Actor',
                description: 'Sam Washington',
            },
        ],
    };
    const genreLabel = film.genreIDs
        ? film.genreIDs.map((genreID) => genres[genreID]).join(', ')
        : null;

    const [isShowingTrailerDialog, setIsShowingTrailerDialog] = useState(false);
    const openTrailerDialog = () => {
        setIsShowingTrailerDialog(true);
    };
    const closeTrailerDialog = () => {
        setIsShowingTrailerDialog(false);
    };

    const [isTrailerLoaded, setIsTrailerLoaded] = useState(false);

    return (
        <React.Fragment>
            {!filmIsLoaded ? (
                <div>Завантаження...</div>
            ) : (
                <React.Fragment>
                    <Modal
                        open={isShowingTrailerDialog}
                        onClose={closeTrailerDialog}
                        className={classes['responsive-trailer-modal']}
                    >
                        <div className={classes['responsive-trailer-wrapper']}>
                            {!isTrailerLoaded && (
                                <div
                                    className={
                                        classes[
                                            'responsive-trailer-loading-overlay'
                                        ]
                                    }
                                >
                                    <CircularProgress
                                        className={
                                            classes[
                                                'responsive-trailer-loading-spinner'
                                            ]
                                        }
                                    />
                                    Завантаження...
                                </div>
                            )}
                            <ReactPlayer
                                className={classes['responsive-trailer-player']}
                                url={film.trailerUrl}
                                controls={true}
                                playing={false}
                                onReady={() => setIsTrailerLoaded(true)}
                            />
                        </div>
                    </Modal>
                    <div
                        className={classes['film-container']}
                        style={{
                            '--background-image-url': `url(${film.featuredImageUrl})`,
                        }}
                    >
                        <div className={classes['film-container-content']}>
                            <div className={classes['film-details']}>
                                <div className={classes['film-metadata']}>
                                    <div
                                        className={classes['film-imdb-rating']}
                                    >
                                        <img src="https://m.media-amazon.com/images/G/01/IMDb/BG_rectangle._CB1509060989_SY230_SX307_AL_.png" />
                                        <span>{film.rating} / 10</span>
                                    </div>
                                    <div>{genreLabel}</div>
                                    <div>
                                        {runtimeMinutesToString(film.runtime)}
                                    </div>
                                    <div>{film.airLanguage}</div>
                                </div>

                                <h1 className={classes['film-title']}>
                                    <strong>{film.title}</strong>
                                </h1>

                                <p className={classes['film-description']}>
                                    {film.description}
                                </p>

                                <div className={classes['film-action-buttons']}>
                                    <Button onClick={openTrailerDialog}>
                                        Дивитися Трейлер
                                    </Button>

                                    <Link to="book">
                                        <Button variant="alternative">
                                            Замовити Квитки
                                        </Button>
                                    </Link>
                                </div>
                            </div>

                            <div className={classes['film-poster-container']}>
                                <img
                                    src={film.posterUrl}
                                    className={classes['film-poster']}
                                />
                            </div>

                            <div
                                className={
                                    classes['film-notable-staff-container']
                                }
                            >
                                {film.notableStaff.map(
                                    (
                                        notableStaff,
                                        notableStaffIndex,
                                        notableStaffCollection
                                    ) => {
                                        const isLast =
                                            notableStaffIndex + 1 ===
                                            notableStaffCollection.length;

                                        return (
                                            <React.Fragment
                                                key={notableStaff.title}
                                            >
                                                <div
                                                    className={
                                                        classes[
                                                            'film-notable-staff-item'
                                                        ]
                                                    }
                                                    key={`notable-staff-item-${notableStaff.title}`}
                                                >
                                                    <img
                                                        className={
                                                            classes[
                                                                'film-notable-staff-image'
                                                            ]
                                                        }
                                                        src={
                                                            notableStaff.picture
                                                        }
                                                    />
                                                    <div>
                                                        <div
                                                            className={
                                                                classes[
                                                                    'film-notable-staff-title'
                                                                ]
                                                            }
                                                        >
                                                            {notableStaff.title}
                                                        </div>
                                                        <div
                                                            className={
                                                                classes[
                                                                    'film-notable-staff-description'
                                                                ]
                                                            }
                                                        >
                                                            {
                                                                notableStaff.description
                                                            }
                                                        </div>
                                                    </div>
                                                </div>
                                                {!isLast && (
                                                    <div
                                                        className={
                                                            classes[
                                                                'film-notable-staff-separator'
                                                            ]
                                                        }
                                                        key={`notable-staff-separator-${notableStaff.title}`}
                                                    ></div>
                                                )}
                                            </React.Fragment>
                                        );
                                    }
                                )}
                            </div>
                        </div>
                    </div>
                </React.Fragment>
            )}
        </React.Fragment>
    );
};

export default Film;
